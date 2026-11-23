namespace QuoteCraft.Helpers;

/// <summary>A platform store for small secrets, by name.</summary>
public interface ISecretStore
{
    string? Read(string name);

    void Write(string name, string value);

    void Delete(string name);
}

/// <summary>
/// One of QuoteCraft's two secrets: the SQLCipher key and the sign-in session (docs/DECISIONS-production-readiness.md,
/// D13). Android, iOS and WinAppSDK keep it in <see cref="Windows.Security.Credentials.PasswordVault"/>; desktop keeps
/// the file in the app's data folder it has always used (Uno has no vault on Skia desktop), and so does WebAssembly,
/// which has no vault either. A file left by an earlier version moves into the vault and is then deleted.
/// </summary>
public sealed class StoredSecret
{
    private readonly string _name;
    private readonly string _file;
    private readonly ISecretStore? _vault;

    /// <param name="name">The secret's name in the vault.</param>
    /// <param name="file">The file that holds it where there is no vault, and that earlier versions wrote everywhere.</param>
    /// <param name="vault">The vault, or null to keep the file (desktop, WebAssembly).</param>
    public StoredSecret(string name, string file, ISecretStore? vault)
    {
        _name = name;
        _file = file;
        _vault = vault;
    }

#if ANDROID || IOS || WINDOWS
    private static readonly Lazy<ISecretStore> PlatformVaultInstance = new(() => new PasswordVaultSecretStore());

    /// <summary>The platform's vault. One instance for the app: Uno's vault writes its whole list at every change.</summary>
    public static ISecretStore? PlatformVault => PlatformVaultInstance.Value;
#else
    /// <summary>No vault on this head (desktop, WebAssembly): secrets stay in files.</summary>
    public static ISecretStore? PlatformVault => null;
#endif

    public string? Read()
    {
        var fromFile = File.Exists(_file) ? File.ReadAllText(_file).Trim() : null;
        if (_vault is null)
        {
            return fromFile;
        }

        if (fromFile is not null)
        {
            // An earlier version's file. It belongs to the data beside it (the database it encrypts), so it wins over
            // anything already in the vault. It is deleted only once the vault gives the same value back.
            _vault.Write(_name, fromFile);
            if (_vault.Read(_name) == fromFile)
            {
                File.Delete(_file);
            }
            return fromFile;
        }

        return _vault.Read(_name);
    }

    public void Write(string value)
    {
        if (_vault is not null)
        {
            _vault.Write(_name, value);
            return;
        }

        if (Path.GetDirectoryName(_file) is { Length: > 0 } folder)
        {
            Directory.CreateDirectory(folder);
        }
        File.WriteAllText(_file, value);
    }

    public void Delete()
    {
        _vault?.Delete(_name);
        if (File.Exists(_file))
        {
            File.Delete(_file);
        }
    }

    /// <summary>The stored secret, or a new one from <paramref name="create"/>, stored before it is returned.</summary>
    public string GetOrCreate(Func<string> create)
    {
        if (Read() is { } existing)
        {
            return existing;
        }

        var value = create();
        Write(value);
        return value;
    }
}

#if ANDROID || IOS || WINDOWS
/// <summary>
/// <see cref="Windows.Security.Credentials.PasswordVault"/>: AndroidKeyStore on Android and the Keychain on iOS (Uno),
/// the Credential Locker on WinAppSDK.
/// </summary>
internal sealed class PasswordVaultSecretStore : ISecretStore
{
    private const string Resource = "QuoteCraft";
    private readonly Windows.Security.Credentials.PasswordVault _vault = new();

    public string? Read(string name)
    {
        if (Find(name) is not { } credential)
        {
            return null;
        }
        credential.RetrievePassword();
        return credential.Password;
    }

    public void Write(string name, string value)
    {
        if (Find(name) is { } existing)
        {
            _vault.Remove(existing);
        }
        _vault.Add(new Windows.Security.Credentials.PasswordCredential(Resource, name, value));
    }

    public void Delete(string name)
    {
        if (Find(name) is { } existing)
        {
            _vault.Remove(existing);
        }
    }

    // RetrieveAll, not Retrieve or FindAllByResource: those throw when nothing matches.
    private Windows.Security.Credentials.PasswordCredential? Find(string name) =>
        _vault.RetrieveAll().FirstOrDefault(c => c.Resource == Resource && c.UserName == name);
}
#endif
