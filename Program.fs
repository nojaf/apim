module Program

open Pulumi.FSharp
open Pulumi.AzureNextGen.Resources.Latest
open Pulumi.AzureNextGen.ApiManagement.Latest

let infra () =
    let stackName = Pulumi.Deployment.Instance.StackName
    // Create an Azure Resource Group
    let resourceGroupName = sprintf "rg-nojaf-apim-%s" stackName

    let resourceGroupArgs =
        ResourceGroupArgs(ResourceGroupName = input resourceGroupName, Location = input "West Europe")

    let resourceGroup =
        ResourceGroup(resourceGroupName, args = resourceGroupArgs)

    let service =
        ApiManagementService
            ("nojaf-apim",
             ApiManagementServiceArgs
                 (ResourceGroupName = io resourceGroup.Name,
                  ServiceName = input "nojaf-apim",
                  Location = input "West Europe",
                  PublisherEmail = input "apim@nojaf.com",
                  PublisherName = input "nojaf",
                  Sku =
                      input
                          (Inputs.ApiManagementServiceSkuPropertiesArgs(Capacity = input 0, Name = input "Consumption"))))

    dict [ ("serviceUrl", box service.GatewayUrl) ]

[<EntryPoint>]
let main _ = Deployment.run infra
