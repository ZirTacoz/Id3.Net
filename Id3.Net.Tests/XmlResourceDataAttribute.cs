#region --- License & Copyright Notice ---
/*
Copyright (c) 2005-2012 Jeevan James
All rights reserved.

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

using Xunit.Sdk;

namespace Id3.Tests
{
    public sealed class XmlResourceDataAttribute : DataAttribute
    {
        private readonly string _resourceName;

        public XmlResourceDataAttribute(string resourceName)
        {
            _resourceName = resourceName;
        }

        public override IEnumerable<object[]> GetData(MethodInfo methodUnderTest)
        {
            Assembly assembly = methodUnderTest.DeclaringType.Assembly;
            using (Stream xmlStream = assembly.GetManifestResourceStream(_resourceName))
            {
                if (xmlStream == null)
                    yield break;

                var xdoc = new XmlDocument();
                xdoc.Load(xmlStream);
                if (xdoc.DocumentElement == null)
                    yield break;

                List<object> attributeValues = null;
                foreach (XmlNode node in xdoc.DocumentElement.ChildNodes)
                {
                    if (node.NodeType != XmlNodeType.Element || node.Attributes == null)
                        continue;
                    if (node.Attributes.Count == 0)
                        continue;
                    if (attributeValues == null)
                        attributeValues = new List<object>(node.Attributes.Count);
                    else
                        attributeValues.Clear();
                    foreach (XmlAttribute attribute in node.Attributes)
                        attributeValues.Add(attribute.Value);
                    yield return attributeValues.ToArray();
                }
            }
        }
    }
}
