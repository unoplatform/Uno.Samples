# WebAuthenticationBroker with OpenID Connect  (OIDC)

This application demonstrates the usage  of the `WebAuthenticationBroker` in Uno with an OpenID Connect endpoint.

The library used for the authentication  is [`IdentityModel.OidcClient`](https://github.com/IdentityModel/IdentityModel.OidcClient), a project backed by the [.NET  foundation](https://dotnetfoundation.org/projects/identitymodel). You can find the documentation [here](https://identitymodel.readthedocs.io/en/latest/native/overview.html).

Important: because of browser restrictions (popup blockers), opening a window must be done during the handling  of the event of a user interaction. So it's preferable to use the *manual  mode* instead of the automatic one and prepare the starting URL in  advance. If you don't care or if you're not planning to use WebAssembly, you can use the *automatic  mode* and implement it directly with the [WAB  Browser](https://identitymodel.readthedocs.io/en/latest/native/overview.html).

Relevant Uno documentation:

* [How to authenticate using OpenID Connect](https://platform.uno/docs/articles/guides/open-id-connect.html)
* [Web Authentication Broker](https://platform.uno/docs/articles/features/web-authentication-broker.html)

This sample is also using the [IdentityServer4's demo Server](https://demo.identityserver.io/). It's also a good choice for the backend of your application. [IdentityServer4 Documentation](https://identityserver4.readthedocs.io/).
