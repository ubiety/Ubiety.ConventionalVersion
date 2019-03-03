/* Copyright 2019 Dieter Lunn
 *
 *   Licensed under the Apache License, Version 2.0 (the "License");
 *   you may not use this file except in compliance with the License.
 *   You may obtain a copy of the License at
 *
 *       http://www.apache.org/licenses/LICENSE-2.0
 *
 *   Unless required by applicable law or agreed to in writing, software
 *   distributed under the License is distributed on an "AS IS" BASIS,
 *   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *   See the License for the specific language governing permissions and
 *   limitations under the License.
 */

using System;

namespace Ubiety.VersionIt.Core.Helpers
{
    /// <summary>
    ///     Helper methods for equality interfaces.
    /// </summary>
    /// <typeparam name="T">Type of instance to help.</typeparam>
    public class EqualityHelper<T>
    {
        private readonly Func<T, object>[] fields;

        /// <summary>
        ///     Initializes a new instance of the <see cref="EqualityHelper{T}"/> class.
        /// </summary>
        /// <param name="fields">Class fields for hash code.</param>
        public EqualityHelper(params Func<T, object>[] fields)
        {
            this.fields = fields;
        }

        /// <summary>
        ///     Determines whether the specified object instances are equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>true if the objects are equal, otherwise false.</returns>
        public bool Equals(T left, T right)
        {
            if (left is null || right is null)
            {
                return false;
            }

            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if (left.GetType() != right.GetType())
            {
                return false;
            }

            foreach (var field in fields)
            {
                if (!Equals(field(left), field(right)))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        ///     Serves as the default hash function.
        /// </summary>
        /// <param name="instance">Object instance to create hash for.</param>
        /// <returns>A hash code for the instance.</returns>
        public int GetHashCode(T instance)
        {
            var hashCode = GetType().GetHashCode();

            unchecked
            {
                foreach (var field in fields)
                {
                    var item = field(instance);
                    hashCode = (hashCode * 450) ^ (item is null ? 0 : item.GetHashCode());
                }
            }

            return hashCode;
        }
    }
}
