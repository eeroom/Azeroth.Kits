using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HzTalk
{
    public class ChatList
    {
        public List<ChatInfo> LstChat { get; set; }
        public ChatList()
        {
            this.LstChat = System.Linq.Enumerable.Range(1, 100)
                .Select(x => new ChatInfo()
                {
                    Img = "img/会话.png",
                    LastTime = new DateTime(2022, 1, 1).AddDays((double)x),
                    User = "张三" + x
                }).ToList();
        }
    }
}
