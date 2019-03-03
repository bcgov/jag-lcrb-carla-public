//---------------------------------------------------------------------
// <copyright file="OpenApiGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using Microsoft.OData.OpenAPI;
using Microsoft.OData.Edm;

namespace OoasUtil
{
    /// <summary>
    /// Open Api generator.
    /// </summary>
    internal abstract class OpenApiGenerator
    {
        /// <summary>
        /// Output format.
        /// </summary>
        public OpenApiTarget Target { get; }

        /// <summary>
        /// Output file.
        /// </summary>
        public string Output { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenApiGenerator"/> class.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="output">The output.</param>
        /// <param name="target">The output target.</param>
        public OpenApiGenerator(string output, OpenApiTarget target)
        {
            Output = output;
            Target = target;
        }

        /// <summary>
        /// Generate the Open Api.
        /// </summary>
        public bool Generate()
        {
            try
            {
                IEdmModel edmModel = GetEdmModel();

                OpenApiWriterSettings settings = GetSettings();

                using (FileStream fs = File.Create(Output))
                {
                    edmModel.WriteOpenApi(fs, Target, settings);
                    fs.Flush();
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        protected virtual OpenApiWriterSettings GetSettings()
        {
            return null;
        }

        protected abstract IEdmModel GetEdmModel();
    }
}
