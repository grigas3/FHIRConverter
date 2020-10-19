﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FHIRConverter.Models
{
    /// <summary>
    /// Fixed properties related to specific tests/observations and resources in general
    /// </summary>
    public class FHIRProperty
    {
        /// <summary>
        /// Resource Path in FHIR Hierarchy
        /// </summary>
        public string ResourcePath { get; set; }

        /// <summary>
        /// Resource Type in FHIR Type
        /// </summary>
        public string ResourceType { get; set; }

        /// <summary>
        /// Value
        /// </summary>
        public string Value { get; set; }


      
    }
}