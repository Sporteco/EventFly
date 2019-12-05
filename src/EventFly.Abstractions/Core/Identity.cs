// The MIT License (MIT)
//
// Copyright (c) 2015-2019 Rasmus Mikkelsen
// Copyright (c) 2015-2019 eBay Software Foundation
// Modified from original source https://github.com/eventflow/EventFlow
//
// Copyright (c) 2018 - 2019 Lutando Ngqakaza
// https://github.com/Lutando/EventFly 
// 
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using EventFly.Extensions;
using EventFly.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace EventFly.Core
{
    public abstract class Identity<T> : SingleValueObject<String>, IIdentity
        where T : Identity<T>
    {
        // ReSharper disable StaticMemberInGenericType
        private static readonly Regex NameReplace = new Regex("Id$");
        public static String IdentityPrefix => NameReplace.Replace(typeof(T).Name, String.Empty).ToLowerInvariant();
        private static readonly Regex ValueValidation = new Regex(
            @"^[a-z0-9]+\-(?<guid>[a-f0-9]{8}\-[a-f0-9]{4}\-[a-f0-9]{4}\-[a-f0-9]{4}\-[a-f0-9]{12})$",
            RegexOptions.Compiled);
        // ReSharper enable StaticMemberInGenericType

        public static T New => With(Guid.NewGuid());

        public static T NewDeterministic(Guid namespaceId, String name)
        {
            var guid = GuidFactories.Deterministic.Create(namespaceId, name);
            return With(guid);
        }

        public static T NewDeterministic(Guid namespaceId, Byte[] nameBytes)
        {
            var guid = GuidFactories.Deterministic.Create(namespaceId, nameBytes);
            return With(guid);
        }

        public static T NewComb()
        {
            var guid = GuidFactories.Comb.Create();
            return With(guid);
        }

        public static T With(String value)
        {
            try
            {
                return (T)Activator.CreateInstance(typeof(T), value);
            }
            catch (TargetInvocationException e)
            {
                if (e.InnerException != null)
                {
                    throw e.InnerException;
                }
                throw;
            }
        }

        public static T With(Guid guid)
        {
            var value = $"{IdentityPrefix}-{guid:D}";
            return With(value);
        }

        public static Boolean IsValid(String value)
        {
            return !Validate(value).Any();
        }

        public static IEnumerable<String> Validate(String value)
        {
            if (String.IsNullOrEmpty(value))
            {
                yield return $"Identity of type '{typeof(T).PrettyPrint()}' is null or empty";
                yield break;
            }

            if (!String.Equals(value.Trim(), value, StringComparison.OrdinalIgnoreCase))
                yield return $"Identity '{value}' of type '{typeof(T).PrettyPrint()}' contains leading and/or traling spaces";
            if (!value.StartsWith(IdentityPrefix))
                yield return $"Identity '{value}' of type '{typeof(T).PrettyPrint()}' does not start with '{IdentityPrefix}'";
            if (!ValueValidation.IsMatch(value))
                yield return $"Identity '{value}' of type '{typeof(T).PrettyPrint()}' does not follow the syntax '[NAME]-[GUID]' in lower case";
        }

        protected Identity(String value)
            : base(value)
        {
            var validationErrors = Validate(value).ToList();
            if (validationErrors.Any())
            {
                throw new ArgumentException($"Identity is invalid: {String.Join(", ", validationErrors)}");
            }

            _lazyGuid = new Lazy<Guid>(() => Guid.Parse(ValueValidation.Match(Value).Groups["guid"].Value));
        }

        private readonly Lazy<Guid> _lazyGuid;

        public Guid GetGuid() => _lazyGuid.Value;
    }
}
