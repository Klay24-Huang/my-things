using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB
{
    public class CarPIC
    {
        /// <summary>
        /// 圖片類型 1:左前;2:右前;3:左後;4:右後;5:電子簽名;6:左前車損;7:右前車損;8:左後車損;9:右後車損
        /// </summary>
        public Int16 ImageType { set; get; }
        /// <summary>
        /// 圖片
        /// </summary>
        public string Image { set; get; }
    }
}
