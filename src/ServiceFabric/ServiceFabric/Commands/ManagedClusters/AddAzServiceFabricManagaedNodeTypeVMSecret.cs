﻿// ----------------------------------------------------------------------------------
// Copyright Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Microsoft.Azure.Commands.Common.KeyVault.Version2016_10_1.Models;
using Microsoft.Azure.Commands.ResourceManager.Common.ArgumentCompleters;
using Microsoft.Azure.Commands.ServiceFabric.Common;
using Microsoft.Azure.Commands.ServiceFabric.Models;
using Microsoft.Azure.Management.Internal.Resources;
using Microsoft.Azure.Management.ServiceFabric;
using Microsoft.Azure.Management.ServiceFabric.Models;

namespace Microsoft.Azure.Commands.ServiceFabric.Commands
{
    [Cmdlet(VerbsCommon.Add, ResourceManager.Common.AzureRMConstants.AzurePrefix + Constants.ServiceFabricPrefix + "ManagedNodeTypeVMSecret", SupportsShouldProcess = true), OutputType(typeof(PSManagedNodeType))]
    public class AddAzServiceFabricManagedNodeTypeVMSecret : ServiceFabricCommonCmdletBase
    {
        #region Params

        #region Common params

        [Parameter(Mandatory = true, Position = 0, ValueFromPipelineByPropertyName = true,
            HelpMessage = "Specify the name of the resource group.")]
        [ResourceGroupCompleter]
        [ValidateNotNullOrEmpty()]
        public string ResourceGroupName { get; set; }

        [Parameter(Mandatory = true, Position = 1, ValueFromPipelineByPropertyName = true,
            HelpMessage = "Specify the name of the cluster.")]
        [ResourceNameCompleter(Constants.ManagedClustersFullType, nameof(ResourceGroupName))]
        [ValidateNotNullOrEmpty()]
        public string ClusterName { get; set; }

        [Parameter(Mandatory = true, Position = 2, ValueFromPipelineByPropertyName = true,
            HelpMessage = "Specify the name of the node type.")]
        [ValidateNotNullOrEmpty()]
        [Alias("NodeTypeName")]
        public string Name { get; set; }

        #endregion

        [Parameter(Mandatory = true, HelpMessage = "Key Vault resouce id containing the certificates.")]
        public string SourceVaultId { get; set; }

        [Parameter(Mandatory = true, HelpMessage = "This is the URL of a certificate that has been uploaded to Key Vault as a secret. For adding a secret to the Key Vault, see [Add a key or secret to the key vault](https://docs.microsoft.com/azure/key-vault/key-vault-get-started/#add). In this case, your certificate needs to be It is the Base64 encoding of the following JSON Object which is encoded in UTF-8: <br><br> {<br>  \"data\":\"<Base64-encoded-certificate>\",<br>  \"dataType\":\"pfx\",<br>  \"password\":\"<pfx-file-password>\"<br>}/")]
        public string CertificateUrl { get; set; }

        [Parameter(Mandatory = true, HelpMessage = "Specifies the certificate store on the Virtual Machine to which the certificate should be added. The specified certificate store is implicitly in the LocalMachine account.")]
        public string CertificateStore { get; set; }

        #endregion

        public override void ExecuteCmdlet()
        {
            if (ShouldProcess(target: this.Name, action: string.Format("Add Secret to node type {0}", this.Name)))
            {
                try
                {
                    NodeType updatedNodeTypeParams = this.GetNodeTypeWithAddedSecret();
                    var beginRequestResponse = this.SFRPClient.NodeTypes.BeginCreateOrUpdateWithHttpMessagesAsync(this.ResourceGroupName, this.ClusterName, this.Name, updatedNodeTypeParams)
                        .GetAwaiter().GetResult();

                    var nodeType = this.PollLongRunningOperation(beginRequestResponse);

                    WriteObject(new PSManagedNodeType(nodeType), false);
                }
                catch (Exception ex)
                {
                    PrintSdkExceptionDetail(ex);
                    throw;
                }
            }
        }

        private NodeType GetNodeTypeWithAddedSecret()
        {
            NodeType currentNodeType = this.SFRPClient.NodeTypes.Get(this.ResourceGroupName, this.ClusterName, this.Name);

            if (currentNodeType.VmSecrets == null)
            {
                currentNodeType.VmSecrets = new List<VaultSecretGroup>();
            }

            var vault = currentNodeType.VmSecrets.FirstOrDefault(v => string.Equals(v.SourceVault.Id, this.SourceVaultId, StringComparison.OrdinalIgnoreCase));
            bool newVaultSecretGroup = false;
            if (vault == null)
            {
                newVaultSecretGroup = true;
                vault = new VaultSecretGroup()
                {
                    SourceVault = new SubResource(this.SourceVaultId)
                };
            }
            
            if (vault.VaultCertificates == null)
            {
                vault.VaultCertificates = new List<VaultCertificate>()
                {
                    new VaultCertificate()
                    {
                        CertificateStore = this.CertificateStore,
                        CertificateUrl = this.CertificateUrl
                    }
                };
            }

            if (newVaultSecretGroup)
            {
                currentNodeType.VmSecrets.Add(vault);
            }

            return currentNodeType;
        }
    }
}
