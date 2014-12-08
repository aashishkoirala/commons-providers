### Commons Library: Provider Set
##### By [Aashish Koirala](http://aashishkoirala.github.io)

This provider set compliments my [commons library](http://aashishkoirala.github.io/commons) and includes certain implementations of interfaces in the commons library. These "providers" can be included and consumed by any application that uses the commons library via MEF and the commons library configuration mechanism.

For more detailed information and documentation, please visit the GitHub page for this repository at [aashishkoirala.github.io/commons-providers](http://aashishkoirala.github.io/commons-providers). You can get each individual provider as a NuGet package [here](https://www.nuget.org/profiles/aashishkoirala/).

###### Update (v1.0.0)
This update consists of:

+ Changes to all providers to fit the new version (1.0.0) of the Commons Library.
+ Removed the web bundling and web SSO providers as they are now defunct in preparation for a new dedicated Commons Web Library. 

###### Update (v0.1.1)
This update consists of the following providers:

+ Data access provider based on MongoDB
+ Configuration store based on Windows Azure Blob Storage
+ Logging provider based on Windows Azure Table Storage

###### Initial Release (v0.1.0)
This version consists of the following providers:

+ Data access provider based on Fluent NHibernate
+ Web bundling provider based on Microsoft's bundling library
+ Web SSO provider based on DotNetOpenAuth - currently only Google is supported
