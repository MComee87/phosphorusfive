/*
 * Phosphorus Five, copyright 2014 - 2017, Thomas Hansen, thomas@gaiasoul.com
 * 
 * This file is part of Phosphorus Five.
 *
 * Phosphorus Five is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License version 3, as published by
 * the Free Software Foundation.
 *
 *
 * Phosphorus Five is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Phosphorus Five.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * If you cannot for some reasons use the GPL license, Phosphorus
 * Five is also commercially available under Quid Pro Quo terms. Check 
 * out our website at http://gaiasoul.com for more details.
 */

using System;
using System.IO;
using System.Security.Cryptography;
using p5.core;
using p5.exp;

namespace phosphorus.crypto
{
    /// <summary>
    ///     Helper class to create Sha256 hash of whatever input given
    /// </summary>
    public static class Hash
    {
        /// <summary>
        ///     Creates a Sha1 hash of input given
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.crypto.hash.create-sha1")]
        public static void p5_crypto_hash_create_sha1 (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution.
            using (new ArgsRemover (e.Args)) {

                // Creating SHA1 hasher.
                using (var sha1 = SHA1.Create ()) {

                    // Invoking worker method.
                    CreateHash (context, sha1, e.Args);
                }
            }
        }

        /// <summary>
        ///     Creates a Sha1 hash of given filename
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.crypto.hash.create-sha1-file")]
        public static void p5_crypto_hash_create_sha1_file (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution.
            using (new ArgsRemover (e.Args)) {

                // Creating SHA1 hasher.
                using (var sha1 = SHA1.Create ()) {

                    // Invoking worker method.
                    CreateHashFromFile (context, sha1, e.Args);
                }
            }
        }

        /// <summary>
        ///     Creates a Sha256 hash of input given
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.crypto.hash.create-sha256")]
        public static void p5_crypto_hash_create_sha256 (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution.
            using (new ArgsRemover (e.Args)) {

                // Creating SHA256 hasher.
                using (var sha1 = SHA256.Create ()) {

                    // Invoking worker method.
                    CreateHash (context, sha1, e.Args);
                }
            }
        }

        /// <summary>
        ///     Creates a Sha256 hash of given filename
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.crypto.hash.create-sha256-file")]
        public static void p5_crypto_hash_create_sha256_file (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution.
            using (new ArgsRemover (e.Args)) {

                // Creating SHA256 hasher.
                using (var sha1 = SHA256.Create ()) {

                    // Invoking worker method.
                    CreateHashFromFile (context, sha1, e.Args);
                }
            }
        }

        /// <summary>
        ///     Creates a SHA512 hash of input given
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.crypto.hash.create-sha512")]
        public static void p5_crypto_hash_create_sha512 (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution.
            using (new ArgsRemover (e.Args)) {

                // Creating SHA512 hasher.
                using (var sha1 = SHA512.Create ()) {

                    // Invoking worker method.
                    CreateHash (context, sha1, e.Args);
                }
            }
        }

        /// <summary>
        ///     Creates a Sha256 hash of given filename
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.crypto.hash.create-sha512-file")]
        public static void p5_crypto_hash_create_sha512_file (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution.
            using (new ArgsRemover (e.Args)) {

                // Creating SHA512 hasher.
                using (var sha1 = SHA512.Create ()) {

                    // Invoking worker method.
                    CreateHashFromFile (context, sha1, e.Args);
                }
            }
        }

        /*
         * Helper methods for above.
         */
        static void CreateHash (ApplicationContext context, HashAlgorithm hasher, Node args)
        {
            // Making sure we clean up and remove all arguments passed in after execution.
            using (new ArgsRemover (args)) {

                // Retrieving value to hash as a blob.
                var whatToHash = XUtil.Single<byte []> (context, args);

                // Checking if caller wants "raw bytes".
                if (args.GetExChildValue ("raw", context, false)) {

                    // Returning hash as raw bytes.
                    args.Value = hasher.ComputeHash (whatToHash);

                } else if (args.GetExChildValue ("hex", context, false)) {

                    // Returning value as hexadecimal string.
                    args.Value = BitConverter.ToString (hasher.ComputeHash (whatToHash)).Replace ("-", string.Empty);

                } else {

                    // Returning hash as base64 encoded string.
                    args.Value = Convert.ToBase64String (hasher.ComputeHash (whatToHash));
                }
            }
        }

        /*
         * Helper method for above.
         */
        static void CreateHashFromFile (ApplicationContext context, HashAlgorithm hasher, Node args)
        {
            // Making sure we clean up and remove all arguments passed in after execution.
            using (new ArgsRemover (args)) {

                // Retrieving filename user wants to hash, unrolling filename, and making sure user has read access to the file.
                var filename = args.GetExValue (context, "");
                filename = context.RaiseEvent ("p5.io.unroll-path", new Node ("", filename).Add ("args", args)).Get<string> (context);
                context.RaiseEvent (".p5.io.authorize.read-file", new Node ("", filename).Add ("args", args));

                // Retrieving root folder of P5.
                var rootFolder = context.RaiseEvent (".p5.core.application-folder").Get<string> (context);

                // Opening file for read access, making sure we dispose it afterwards.
                using (var stream = File.OpenRead (rootFolder + filename)) {

                    // Checking if caller wants "raw bytes".
                    if (args.GetExChildValue ("raw", context, false)) {

                        // Returning hash as raw bytes.
                        args.Value = hasher.ComputeHash (stream);

                    } else if (args.GetExChildValue ("hex", context, false)) {

                        // Returning value as hexadecimal string.
                        args.Value = BitConverter.ToString (hasher.ComputeHash (stream)).Replace ("-", string.Empty);

                    } else {

                        // Returning hash as base64 encoded string.
                        args.Value = Convert.ToBase64String (hasher.ComputeHash (stream));
                    }
                }
            }
        }
    }
}
