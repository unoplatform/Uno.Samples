# Text Features Showcase

Text Features Showcase is a single-page [Uno Platform](https://platform.uno) app that demonstrates the text-rendering wave shipped in **Uno Platform 6.6**. Everything runs on the Skia rendering engine across Web (WebAssembly), Windows, Linux, macOS, iOS, and Android.

The sample brings together five 6.6 features:

- **TextBox spell checking** — [PR #22383](https://github.com/unoplatform/uno/pull/22383)
- **TextBlock TextHighlighters** — [PR #22448](https://github.com/unoplatform/uno/pull/22448)
- **TextBlock TextTrimming** — [PR #22572](https://github.com/unoplatform/uno/pull/22572)
- **Sophisticated font fallback** — [PR #22240](https://github.com/unoplatform/uno/pull/22240)
- **FeatureConfiguration.TextBlock.RenderWhiteSpace** — [PR #22761](https://github.com/unoplatform/uno/pull/22761)

![Text Features Showcase](doc/assets/TextFeaturesShowcase.png)

## Features shown

- A `TextBox` with `IsSpellCheckEnabled="True"` drawing red squiggles under real misspelled words.
- A `TextBlock` using `TextHighlighters` to paint live search matches as you type.
- Three `TextBlock`s at a fixed width comparing `TextTrimming` values `CharacterEllipsis`, `WordEllipsis`, and `None` side by side.
- A single `TextBlock` mixing Latin, emoji, and CJK scripts to show sophisticated font fallback with no missing-glyph (tofu) boxes.
- A `ToggleSwitch` bound to `FeatureConfiguration.TextBlock.RenderWhiteSpace` to visualize whitespace-glyph rendering.

## Codebase

* [**MainPage.xaml**](MainPage.xaml): The full single-page UI — one section per 6.6 feature, themed with `{ThemeResource}` brushes so it works in both light and dark modes.
* [**MainPage.xaml.cs**](MainPage.xaml.cs): Wires the interactive bits — live `TextHighlighter` search over character ranges, and the `RenderWhiteSpace` toggle that flips the `FeatureConfiguration` flag and re-renders the sample text.

## Requirements

- The spell-checking feature requires the **`SpellChecking`** `<UnoFeatures>` entry in the project file (pulls in the `Uno.WinUI.SpellChecking` package).

## What is the Uno Platform

[Uno Platform](https://platform.uno) is an open-source .NET platform for building single codebase native mobile, web, desktop, and embedded apps quickly.
For additional information about Uno Platform or if you have any feedback to share, please refer to the [README.md](../../README.md) file in this Samples repository.
