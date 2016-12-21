using System;

using LMS.Model;
using LMS.Model.Resource;

namespace LMS.Data
{
    public class ArtistValidator : DataValidator<Artist>
    {
        public ArtistValidator()
            : base()
        {
        }

        public override DataValidationResult Validate(Artist item)
        {
            return base.Validate(item);
        }
    }
}
