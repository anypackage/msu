// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using System;

namespace AnyPackage.Provider.Msu
{
    [PackageProvider("Msu")]
    public sealed class MsuProvider : PackageProvider
    {
        private readonly static Guid s_id = new Guid("314633fe-c7e9-4eeb-824b-382a8a4e92b8");

        public MsuProvider() : base(s_id) { }
    }
}
