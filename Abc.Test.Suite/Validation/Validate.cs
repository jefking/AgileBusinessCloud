// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='Validate.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Validation
{
    using System.Collections.Generic;
    using Abc.Services.Validation;

    public class Validate : IValidate<Validate>
    {
        public bool Valid
        {
            get;
            set;
        }

        public IEnumerable<Rule<Validate>> Rules
        {
            get
            {
                return new Rule<Validate>[] { new Rule<Validate>(c => c.Valid, "Invalid") };
            }
        }
    }
}