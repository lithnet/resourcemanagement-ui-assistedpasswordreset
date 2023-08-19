![](https://lithnet.github.io/images/logo-ex-small.png)
# Assisted password reset module for FIM 2010/MIM 2016
The Lithnet Assisted Password Reset (APR) module is an extension to the FIM/MIM portal that provides the ability for your help desk staff to reset a user's password directly from the MIM portal.

![](https://github.com/lithnet/resourcemanagement-ui-assistedpasswordreset/wiki/images/screen-shot1.png)

## Features
1. Can create randomly generated passwords of a configurable length, or
2. Allows password to be specified by the operator
3. Includes the ability to force the user to change their password at the next login
4. Optionally, can require operator to re-authenticate to reset a user's password
5. Customize the list of attributes shown on the page
6. Supports localized attribute names from the FIM/MIM service
7. Integrates with the user RCDC using a UocHyperLink control
8. Resets passwords directly against the AD, utilizing existing permission delegation

## Requirements
* SharePoint 2013 or later
* FIM 2010 R2 or later
* .NET Framework 4.0 or later

## Getting started
The module is a simple WSP package that needs to be deployed into your SharePoint farm. The [installation guide](https://github.com/lithnet/resourcemanagement-ui-assistedpasswordreset/wiki/Installation-and-upgrade-steps) covers all the steps for installing and upgrading the module

## Localization
The module will match the locale of the MIM portal, rendering attribute display names in the browser's preferred language. The module itself can be localized into any language with a provided translation.

If you want to contribute a translation, see the "How can I contribute" section below.

## How can I contribute to the project?
* Found an issue and want us to fix it? [Log it](https://github.com/lithnet/resourcemanagement-ui-assistedpasswordreset/issues)
* Want to fix an issue yourself or add functionality? Clone the project and submit a pull request

## Enterprise support
Lithnet offer enterprise support plans for our open-source products. Deploy our tools with confidence that you have the backing of the dedicated Lithnet support team if you run into any issues, have questions, or need advice. Simply fill out the [request form](https://lithnet.io/products/mim), let us know the number of users you are managing with your MIM implementation, and we'll put together a quote.

## Keep up to date
* [Visit our blog](http://blog.lithnet.io)
* [Follow us on twitter](https://twitter.com/lithnet_io)![](http://twitter.com/favicon.ico)
