# Skyline.DataMiner.Utils.ExposerWidgets

## About

### About Skyline.DataMiner.Utils.ExposerWidgets

This solution contains all widgets and sections representing filters that can be used to find all kinds of objects in the DataMiner system, e.g.: DOM instances, reservations instances, tickets, jobs, ...
The solution is released as a public Nuget so every automation script can implement these widgets and sections.

### About DataMiner

DataMiner is a transformational platform that provides vendor-independent control and monitoring of devices and services. Out of the box and by design, it addresses key challenges such as security, complexity, multi-cloud, and much more. It has a pronounced open architecture and powerful capabilities enabling users to evolve easily and continuously.

The foundation of DataMiner is its powerful and versatile data acquisition and control layer. With DataMiner, there are no restrictions to what data users can access. Data sources may reside on premises, in the cloud, or in a hybrid setup.

A unique catalog of 7000+ connectors already exists. In addition, you can leverage DataMiner Development Packages to build your own connectors (also known as "protocols" or "drivers").

> [!NOTE]
> See also: [About DataMiner](https://aka.dataminer.services/about-dataminer).

### About Skyline Communications

At Skyline Communications, we deal with world-class solutions that are deployed by leading companies around the globe. Check out [our proven track record](https://aka.dataminer.services/about-skyline) and see how we make our customers' lives easier by empowering them to take their operations to the next level.

## Requirements

The "DataMiner Integration Studio" Visual Studio extension is required for development of connectors and Automation scripts using NuGets.

See [Installing DataMiner Integration Studio](https://aka.dataminer.services/DisInstallation)

> [!IMPORTANT]  
> The sections provided by this Nuget can currently only be used in automation scripts that use the [InteractiveAutomationScriptToolkit](https://github.com/SkylineCommunications/Skyline.DataMiner.Utils.InteractiveAutomationScriptToolkit) range 6.0.X, which requires DataMiner version 10.3.1 or higher.

## User Guide

To get started, the `Skyline.DataMiner.Utils.ExposerWidgets` NuGet package needs to be added to the C# project from [nuget.org](https://www.nuget.org/packages/Skyline.DataMiner.Utils.ExposerWidgets)
For more information see https://docs.dataminer.services/develop/TOOLS/NuGet/Consuming_NuGet.html.

### Configuring Filters

See the screenshot below for more details.

1. **Including/excluding filters** can be done by checking or unchecking the checkbox next to the item property name.

1. **Filter type** can be selected using the dropdowns marked with number 2. Different properties allow different filter types depending on their possible values.

1. **Filter value** can be filled in in the boxes marked with number 3. Depending on the property that is being filtered on, this will we strings, integers, Guids or datetimes.

1. **Adding more filters** can be done using the duplicate button, marked with number 4. There is no limit on the amount of filters you can include.

1. **Executing the query** with the included filters can be done using the button at the bottom. Marked in the screenshot below with number 5.

> [!IMPORTANT]  
> All included filters will be combined with an AND operation. There is currently no way of adding OR filters.

![configuring filters](Filters.png "Configuring Filters")



### Finetuning results

Once the items are retrieved based on the filters, there is the possibility to finetune the selection manually. As can be seen in the screenshot below.

1. The **amount of matching items** is displayed to give a quick view on how many total items are in the system that match the filters.

1. **Selecting/unselecting all items** can be done using the buttons marked with number 2.

1. The **amount of selected items** is also displayed to keep the user aware of his current selection.

1. A scrollable checkbox list allows users to **individually select/unselect** items to finetune the result to their wishes.

![finetuning results](Results.png "Finetuning Results")

