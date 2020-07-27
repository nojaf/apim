module Program

open Pulumi.FSharp
open Pulumi.Azure.Core
open Pulumi.Azure

let infra () =
    let stackName = Pulumi.Deployment.Instance.StackName
    // Create an Azure Resource Group
    let resourceGroupName = sprintf "rg-nojaf-apim-%s" stackName
    let resourceGroupArgs = ResourceGroupArgs(Name = input resourceGroupName)
    let resourceGroup = ResourceGroup(resourceGroupName, args = resourceGroupArgs)

    let arm = """
    {
        "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
        "contentVersion": "1.0.0.0",
        "resources": [{
            "apiVersion": "2019-12-01",
            "name": "nojaf-apim",
            "type": "Microsoft.ApiManagement/service",
            "location": "westeurope",
            "tags": {},
            "sku": {
                "name": "Consumption",
                "capacity": "0"
            },
            "properties": {
                "publisherEmail": "apim@nojaf.com",
                "publisherName": "nojaf"
            }
        }]
    }
    """

    let deployment = TemplateDeployment("nojaf-apim", TemplateDeploymentArgs(ResourceGroupName = io resourceGroup.Name,
                                                                                      TemplateBody = input arm,
                                                                                      DeploymentMode = input "Incremental"))


    dict [ ("deploymentId", box deployment.Id) ]

[<EntryPoint>]
let main _ =
  Deployment.run infra
