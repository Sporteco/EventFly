﻿// The MIT License (MIT)
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

using FluentValidation.Results;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace EventFly.Commands.ExecutionResults
{
    public class FailedExecutionResult : ExecutionResult
    {
        public virtual IReadOnlyCollection<System.String> Errors { get; }

        [JsonConstructor]
        public FailedExecutionResult(IEnumerable<System.String> errors)
        {
            Errors = (errors ?? Enumerable.Empty<System.String>()).ToList();
        }

        public FailedExecutionResult(System.String error) : this(new[] { error }) { }

        public override System.Boolean IsSuccess { get; } = false;

        public override System.String ToString()
        {
            return Errors.Any()
                ? $"Failed execution due to: {System.String.Join(", ", Errors)}"
                : "Failed execution";
        }
    }

    public sealed class UnhandledExceptionCommandResult : FailedExecutionResult
    {
        public UnhandledExceptionCommandResult(System.String message, System.String stackTrace) : base(message)
        {
            StackTrace = stackTrace;
        }

        public System.String StackTrace { get; }
    }

    public sealed class UnauthorizedAccessResult : FailedExecutionResult
    {
        public UnauthorizedAccessResult()
            : base("Unauthorized access")
        {
        }
    }

    public class FailedValidationExecutionResult : FailedExecutionResult
    {
        public ValidationResult ValidationResult { get; }

        public FailedValidationExecutionResult(ValidationResult validationResult) : base(validationResult.Errors.Select(i => i.ErrorMessage).ToList())
        {
            ValidationResult = validationResult;
        }
    }
}