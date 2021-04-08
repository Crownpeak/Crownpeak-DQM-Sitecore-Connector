![Cognifide logo](docs/Logo.png)

[![Apache License, Version 2.0, January 2004](https://img.shields.io/github/license/Cognifide/gradle-aem-plugin.svg?label=License)](http://www.apache.org/licenses/)

# Crownpeak DQM Sitecore Connector

Crownpeak DQM Sitecore connector builds Digital Quality Management directly into your Sitecore publication workflow, closing the gap between content authoring and digital quality assurance. So your digital marketing team can test and optimize content without ever leaving the Sitecore environment.

Organizations with multiple websites, multiple channels and globally-distributed digital teams need control over the quality and consistency of every digital touchpoint.  Otherwise, issues ranging from broken links to outdated content or SEO errors can go undetected, and become serious and costly -- or worse, compromise the integrity of your brand and put your organization at risk. 

Crownpeak DQM protects your brand and your organization by crawling your site and identifying issues based on a set of best practice checkpoints as well your own customizable list.

Learn more about the detailed features of the product and why the world’s top brands rely on the leading SaaS solution for digital governance, Crownpeak DQM.

## Requirements

* Sitecore  8.2
* Visual Studio 2017

## Deployment

Conventional deployment requires two steps:

1. Deploy project using publish feature in Visual Studio
2. Run [Unicorn](https://github.com/kamsar/Unicorn) synchronization

Creating Sitecore package

1. Install [Sitecore Powershell Extensions](https://marketplace.sitecore.net/en/Modules/Sitecore_PowerShell_console.aspx)
2. Run `CreatePackage.ps1` file in Powerhshell ISE
3. Download created package

## License

**Crownpeak DQM Sitecore Connector** is licensed under the [Apache License, Version 2.0 (the "License")](https://www.apache.org/licenses/LICENSE-2.0.txt)


