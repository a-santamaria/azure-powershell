// <auto-generated>
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for
// license information.
//
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Microsoft.Azure.Commands.Common.Compute.Version2016_04_preview.Models
{
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Describes a virtual machine scale set storage profile.
    /// </summary>
    public partial class VirtualMachineScaleSetStorageProfile
    {
        /// <summary>
        /// Initializes a new instance of the
        /// VirtualMachineScaleSetStorageProfile class.
        /// </summary>
        public VirtualMachineScaleSetStorageProfile()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// VirtualMachineScaleSetStorageProfile class.
        /// </summary>
        /// <param name="imageReference">The image reference.</param>
        /// <param name="osDisk">The OS disk.</param>
        /// <param name="dataDisks">The data disks.</param>
        public VirtualMachineScaleSetStorageProfile(ImageReference imageReference = default(ImageReference), VirtualMachineScaleSetOSDisk osDisk = default(VirtualMachineScaleSetOSDisk), IList<VirtualMachineScaleSetDataDisk> dataDisks = default(IList<VirtualMachineScaleSetDataDisk>))
        {
            ImageReference = imageReference;
            OsDisk = osDisk;
            DataDisks = dataDisks;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets the image reference.
        /// </summary>
        [JsonProperty(PropertyName = "imageReference")]
        public ImageReference ImageReference { get; set; }

        /// <summary>
        /// Gets or sets the OS disk.
        /// </summary>
        [JsonProperty(PropertyName = "osDisk")]
        public VirtualMachineScaleSetOSDisk OsDisk { get; set; }

        /// <summary>
        /// Gets or sets the data disks.
        /// </summary>
        [JsonProperty(PropertyName = "dataDisks")]
        public IList<VirtualMachineScaleSetDataDisk> DataDisks { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="Rest.ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (OsDisk != null)
            {
                OsDisk.Validate();
            }
            if (DataDisks != null)
            {
                foreach (var element in DataDisks)
                {
                    if (element != null)
                    {
                        element.Validate();
                    }
                }
            }
        }
    }
}
