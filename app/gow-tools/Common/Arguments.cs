﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Text.RegularExpressions;


namespace gow.tools.Common
    {
    /// <summary>
    /// Arguments class
    /// </summary>
    public class Arguments
        {
        // Variables
        private readonly StringDictionary _parameters;

        // Constructor
        public Arguments(string[] args)
            {
            _parameters = new StringDictionary();
            Regex spliter = new Regex(@"^-{1,2}|^/|=|:",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);

            Regex remover = new Regex(@"^['""]?(.*?)['""]?$",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);

            string parameter = null;
            string[] Parts;

            // Valid parameters forms:
            // {-,/,--}param{ ,=,:}((",')value(",'))
            // Examples: 
            // -param1 value1 --param2 /param3:"Test-:-work" 
            //   /param4=happy -param5 '--=nice=--'
            foreach (string Txt in args)
                {
                // Look for new parameters (-,/ or --) and a
                // possible enclosed value (=,:)
                Parts = spliter.Split(Txt, 3);

                switch (Parts.Length)
                    {
                    // Found a value (for the last parameter 
                    // found (space separator))
                    case 1:
                        if (parameter != null)
                            {
                            if (!_parameters.ContainsKey(parameter))
                                {
                                Parts[0] =
                                    remover.Replace(Parts[0], "$1");

                                _parameters.Add(parameter, Parts[0]);
                                }
                            parameter = null;
                            }
                        // else Error: no parameter waiting for a value (skipped)
                        break;

                    // Found just a parameter
                    case 2:
                        // The last parameter is still waiting. 
                        // With no value, set it to true.
                        if (parameter != null)
                            {
                            if (!_parameters.ContainsKey(parameter))
                                _parameters.Add(parameter, "true");
                            }
                        parameter = Parts[1];
                        break;

                    // Parameter with enclosed value
                    case 3:
                        // The last parameter is still waiting. 
                        // With no value, set it to true.
                        if (parameter != null)
                            {
                            if (!_parameters.ContainsKey(parameter))
                                _parameters.Add(parameter, "true");
                            }

                        parameter = Parts[1];

                        // Remove possible enclosing characters (",')
                        if (!_parameters.ContainsKey(parameter))
                            {
                            Parts[2] = remover.Replace(Parts[2], "$1");
                            _parameters.Add(parameter, Parts[2]);
                            }

                        parameter = null;
                        break;
                    }
                }
            // In case a parameter is still waiting
            if (parameter != null)
                {
                if (!_parameters.ContainsKey(parameter))
                    _parameters.Add(parameter, "true");
                }
            }

        // Retrieve a parameter value if it exists 
        // (overriding C# indexer property)
        public string this[string Param]
            {
            get
                {
                return (_parameters[Param]);
                }
            }
        }
    }
