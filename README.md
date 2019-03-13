# ![Logo](https://github.com/ubiety/Ubiety.VersionIt/raw/develop/images/version64.png) Ubiety VersionIt ![Nuget](https://img.shields.io/nuget/v/Ubiety.VersionIt.svg?style=flat-square)

> VersionIt takes the pain out of maintaining your version and changelog. Just commit your code and your code will version itself.

![Tidelift Dependencies](https://tidelift.com/badges/github/ubiety/Ubiety.VersionIt)

[Professionally supported Ubiety.VersionIt is coming soon](https://tidelift.com/subscription/pkg/nuget-ubiety-versionit?utm_source=nuget-ubiety-versionit&utm_medium=referral&utm_campaign=readme)

| Branch  | Quality                                                                                                                                                                                                    | Travis CI                                                                                                                                                     | Appveyor                                                                                                                                                                                     | Coverage                                                                                                                                                                     |
| ------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Master  | [![CodeFactor](https://www.codefactor.io/repository/github/ubiety/ubiety.versionit/badge?style=flat-square)](https://www.codefactor.io/repository/github/ubiety/ubiety.versionit)                          | [![Travis (.org) branch](https://img.shields.io/travis/ubiety/Ubiety.VersionIt/master.svg?style=flat-square)](https://travis-ci.org/ubiety/Ubiety.VersionIt)  | [![AppVeyor branch](https://img.shields.io/appveyor/ci/coder2000/ubiety-versionit/master.svg?style=flat-square)](https://ci.appveyor.com/project/coder2000/ubiety-versionit/branch/master)   | [![Coveralls github branch](https://img.shields.io/coveralls/github/ubiety/Ubiety.VersionIt/master.svg?style=flat-square)](https://coveralls.io/github/ubiety/Ubiety.VersionIt?branch=master)   |
| Develop | [![CodeFactor](https://www.codefactor.io/repository/github/ubiety/ubiety.versionit/badge/develop?style=flat-square)](https://www.codefactor.io/repository/github/ubiety/ubiety.versionit/overview/develop) | [![Travis (.org) branch](https://img.shields.io/travis/ubiety/Ubiety.VersionIt/develop.svg?style=flat-square)](https://travis-ci.org/ubiety/Ubiety.VersionIt) | [![AppVeyor branch](https://img.shields.io/appveyor/ci/coder2000/ubiety-versionit/develop.svg?style=flat-square)](https://ci.appveyor.com/project/coder2000/ubiety-versionit/branch/develop) | [![Coveralls github branch](https://img.shields.io/coveralls/github/ubiety/Ubiety.VersionIt/develop.svg?style=flat-square)](https://coveralls.io/github/ubiety/Ubiety.VersionIt?branch=develop) |

VersionIt allows you to maintain your changelog and project version with ease. VersionIt follows both the conventional commit and GitFlow standards.

## Installing / Getting started

Ubiety VersionIt can be installed as a dotnet tool or MSBuild task

```shell
dotnet tool install --global Ubiety.VersionIt
```

```shell
dotnet add package Ubiety.VersionIt.MSBuild
```

You can also use your favorite NuGet client.

## Developing

Here's a brief intro about what a developer must do in order to start developing
the project further:

```shell
git clone https://github.com/ubiety/Ubiety.VersionIt.git
cd Ubiety.VersionIt
dotnet restore
```

Clone the repository and then restore the development requirements. You can use
any editor, Rider, VS Code or VS 2017. The library supports all .NET Core
platforms.

### Building

Building is simple

```shell
dotnet build
```

### Deploying / Publishing

```shell
git pull
versionize
dotnet pack
dotnet nuget push
git push
```

## Contributing

When you publish something open source, one of the greatest motivations is that
anyone can just jump in and start contributing to your project.

These paragraphs are meant to welcome those kind souls to feel that they are
needed. You should state something like:

"If you'd like to contribute, please fork the repository and use a feature
branch. Pull requests are warmly welcome."

If there's anything else the developer needs to know (e.g. the code style
guide), you should link it here. If there's a lot of things to take into
consideration, it is common to separate this section to its own file called
`CONTRIBUTING.md` (or similar). If so, you should say that it exists here.

## Links

- Project homepage: <https://versionit.dieterlunn.ca>
- Repository: <https://github.com/ubiety/Ubiety.VersionIt/>
- Issue tracker: <https://github.com/ubiety/Ubiety.VersionIt/issues>
  - In case of sensitive bugs like security vulnerabilities, please use the 
    [Tidelift security contact](https://tidelift.com/security) instead of using issue tracker. 
    We value your effort to improve the security and privacy of this project! Tidelift will coordinate the fix and disclosure.
- Related projects:
  - Ubiety Xmpp: <https://github.com/ubiety/Ubiety.Xmpp.Core/>
  - Ubiety Toolset: <https://github.com/ubiety/Ubiety.Toolset/>
  - Ubiety Dns: <https://github.com/ubiety/Ubiety.Dns.Core/>
  - Ubiety Stringprep: <https://github.com/ubiety/Ubiety.Stringprep.Core/>
  - Ubiety SCRAM: <https://github.com/ubiety/Ubiety.Scram.Core/>

## Sponsors

### Gold Sponsors

[![Gold Sponsors](https://opencollective.com/ubiety/tiers/gold-sponsor.svg?avatarHeight=36)](https://opencollective.com/ubiety/)

### Silver Sponsors

[![Silver Sponsors](https://opencollective.com/ubiety/tiers/silver-sponsor.svg?avatarHeight=36)](https://opencollective.com/ubiety/)

### Bronze Sponsors

[![Bronze Sponsors](https://opencollective.com/ubiety/tiers/bronze-sponsor.svg?avatarHeight=36)](https://opencollective.com/ubiety/)

## Licensing

The code in this project is licensed under the [Apache 2.0](https://choosealicense.com/licenses/apache-2.0/).
