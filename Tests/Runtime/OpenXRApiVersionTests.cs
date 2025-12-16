using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace UnityEngine.XR.OpenXR.Tests
{
    class OpenXRApiVersionTests
    {
        internal struct TestCase<TExpected>
        {
            public OpenXRApiVersion a;
            public OpenXRApiVersion b;
            public TExpected expectedResult;

            public override readonly string ToString()
            {
                return string.Join(',',
                    a?.ToString() ?? "null", b?.ToString() ?? "null", $"expected = {expectedResult}");
            }
        }

        static readonly List<TestCase<int>> s_compareToTestCases = new()
        {
            // Null case
            new() { a = new(1, 0, 0), b = null, expectedResult = 1 },
            // Major version
            new() { a = new(1, 0, 0), b = new(1, 0, 0), expectedResult = 0 },
            new() { a = new(2, 0, 0), b = new(1, 0, 0), expectedResult = 1 },
            new() { a = new(1, 0, 0), b = new(2, 0, 0), expectedResult = -1 },
            // Minor version
            new() { a = new(1, 2, 0), b = new(1, 2, 0), expectedResult = 0 },
            new() { a = new(1, 2, 0), b = new(1, 0, 0), expectedResult = 1 },
            new() { a = new(1, 0, 0), b = new(1, 2, 0), expectedResult = -1 },
            // Patch version
            new() { a = new(1, 2, 3), b = new(1, 2, 3), expectedResult = 0 },
            new() { a = new(1, 2, 3), b = new(1, 2, 0), expectedResult = 1 },
            new() { a = new(1, 2, 0), b = new(1, 2, 3), expectedResult = -1 }
        };

        [Test]
        public void OpenXRApiVersion_CompareTo(
            [ValueSource(nameof(s_compareToTestCases))] TestCase<int> testCase)
        {
            Assert.AreEqual(testCase.expectedResult, testCase.a.CompareTo(testCase.b));
        }

        static readonly List<TestCase<bool>> s_equalsTestCases = new()
        {
            // Null case
            new() { a = new(1, 0, 0), b = null, expectedResult = false },
            // Major version
            new() { a = new(1, 0, 0), b = new(1, 0, 0), expectedResult = true },
            new() { a = new(2, 0, 0), b = new(1, 0, 0), expectedResult = false },
            new() { a = new(1, 0, 0), b = new(2, 0, 0), expectedResult = false },
            // Minor version
            new() { a = new(1, 2, 0), b = new(1, 2, 0), expectedResult = true },
            new() { a = new(1, 2, 0), b = new(1, 0, 0), expectedResult = false },
            new() { a = new(1, 0, 0), b = new(1, 2, 0), expectedResult = false },
            // Patch version
            new() { a = new(1, 2, 3), b = new(1, 2, 3), expectedResult = true },
            new() { a = new(1, 2, 3), b = new(1, 2, 0), expectedResult = false },
            new() { a = new(1, 2, 0), b = new(1, 2, 3), expectedResult = false }
        };

        [Test]
        public void OpenXRApiVersion_Equals(
            [ValueSource(nameof(s_equalsTestCases))] TestCase<bool> testCase)
        {
            Assert.AreEqual(testCase.expectedResult, testCase.a.Equals(testCase.b));
        }
    }
}
