// Copyright 2004-2021 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.DynamicProxy.Tests.Classes
{
	using System;

	public class RequiredAttribute : Attribute
	{
		private readonly object defaultValue;
		private readonly bool hasDefault;
		public object BadValue;

		public RequiredAttribute()
		{
		}

		public RequiredAttribute(object defaultValue)
		{
			hasDefault = true;
			this.defaultValue = defaultValue;
		}

		public object DefaultValue
		{
			get
			{
				if (!hasDefault)
				{
					throw new ArgumentException("No default value for argument");
				}
				return defaultValue;
			}
		}
	}
}