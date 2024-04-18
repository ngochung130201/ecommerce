using System.ComponentModel;

namespace ecommerce.Enums
{
    public enum AgeRange
    {
        None = 0,
        [Description("Từ 0 - 12 tháng")]
        From0To12Months,

        [Description("Từ 1 - 3 tuổi")]
        From1To3Years,

        [Description("Từ 3 - 6 tuổi")]
        From3To6Years,

        [Description("Từ 6 - 12 tuổi")]
        From6To12Years,

        [Description("Từ 12 tuổi trở nên")]
        From12YearsAndAbove
    }

}
