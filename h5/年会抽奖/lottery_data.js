var lottery_initial_datas =[
    	{
        "nameen": "avatar1",
        "namezh": "周张林"
       	},
        {
        "nameen": "avatar2",
        "namezh": "李冬"
        }
        ,
        {
        "nameen": "avatar3",
        "namezh": "曾菁"
        },
        {
        "nameen": "avatar4",
        "namezh": "倪澄新"
        }
        ,
        {
        "nameen": "avatar5",
        "namezh": "林冬梅"
        }
        ,
        {
        "nameen": "avatar6",
        "namezh": "潘春燕"
        }
        ,
        {
        "nameen": "avatar7",
        "namezh": "张静怡"
        },
        {
        "nameen": "avatar8",
        "namezh": "巫书瑶"
        },
        {
        "nameen": "avatar9",
        "namezh": "杨钢"
        },   
        {
        "nameen": "avatar10",
        "namezh": "宋文琦"
        },
        {
        "nameen": "avatar11",
        "namezh": "张鹏"
        },
        {
        "nameen": "avatar12",
        "namezh": "谌丹丹"
        },
        {
        "nameen": "avatar13",
        "namezh": "侯皝"
        },
        {
        "nameen": "avatar14",
        "namezh": "冯琳芝"
        },
        {
        "nameen": "avatar15",
        "namezh": "邹文"
        },
        {
        "nameen": "avatar16",
        "namezh": "罗晓鹏"
        },
        {
        "nameen": "avatar17",
        "namezh": "黄瑞"
        },
        {
        "nameen": "avatar18",
        "namezh": "黄文"
        },
        {
        "nameen": "avatar19",
        "namezh": "王茂遥"
        },
        {
        "nameen": "avatar20",
        "namezh": "胡婉逸"
        },
        {
        "nameen": "avatar21",
        "namezh": "许泽沛"
        },
        {
        "nameen": "avatar22",
        "namezh": "陈芳"
        },
        {
        "nameen": "avatar23",
        "namezh": "唐金龙"
        },
        {
        "nameen": "avatar24",
        "namezh": "付丽馨"
        },
        {
        "nameen": "avatar25",
        "namezh": "黄文佳"
        },
        {
        "nameen": "avatar26",
        "namezh": "李元"
        },
        {
        "nameen": "avatar27",
        "namezh": "罗斯佳"
        },
        {
        "nameen": "avatar28",
        "namezh": "罗攀"
        },
        {
        "nameen": "avatar29",
        "namezh": "雷玲玲"
        },
        {
        "nameen": "avatar30",
        "namezh": "刘梦媛"
        },
        {
        "nameen": "avatar31",
        "namezh": "王东"
        },
        {
        "nameen": "avatar32",
        "namezh": "陈果"
        },
        {
        "nameen": "avatar33",
        "namezh": "王东"
        },
        {
        "nameen": "avatar34",
        "namezh": "李巽"
        },
        {
        "nameen": "avatar35",
        "namezh": "刘静雯"
        },
        {
        "nameen": "avatar36",
        "namezh": "苟谆"
        },
        {
        "nameen": "avatar37",
        "namezh": "马宁"
        },
        {
        "nameen": "avatar38",
        "namezh": "毛雨东"
        },
        {
        "nameen": "avatar39",
        "namezh": "罗伟军"
        },
        {
        "nameen": "avatar40",
        "namezh": "王卓瑞"
        },
        {
        "nameen": "avatar41",
        "namezh": "陈科"
        },
        {
        "nameen": "avatar42",
        "namezh": "唐伟"
        },
        {
        "nameen": "avatar43",
        "namezh": "刘璐莹"
        },
        {
        "nameen": "avatar44",
        "namezh": "宋振中"
        },
        {
        "nameen": "avatar45",
        "namezh": "王婷"
        },
        {
        "nameen": "avatar46",
        "namezh": " 许兴宇"
        },
        {
        "nameen": "avatar47",
        "namezh": "易满"
        },
        {
        "nameen": "avatar48",
        "namezh": "杨春"
        },
        {
        "nameen": "avatar49",
        "namezh": "王楚月"
        },
        {
        "nameen": "avatar50",
        "namezh": "贺小龙"
        },
        {
        "nameen": "avatar51",
        "namezh": "牛珂欣"
        },
        {
        "nameen": "avatar52",
        "namezh": "王秀雷"
        },
        {
        "nameen": "avatar53",
        "namezh": "吴雨桐"
        },
        {
        "nameen": "avatar54",
        "namezh": "王宇乔"
        },
        {
        "nameen": "avatar55",
        "namezh": "曹文杰"
        },
        {
        "nameen": "avatar56",
        "namezh": "朱若瑜"
        },
        {
        "nameen": "avatar57",
        "namezh": "曾恬甜"
        },
        {
        "nameen": "avatar58",
        "namezh": "程莉娟"
        },
        {
        "nameen": "avatar59",
        "namezh": "罗娜"
        },
        {
        "nameen": "avatar60",
        "namezh": "徐圣超"
        },
        {
        "nameen": "avatar61",
        "namezh": "张英驰"
        },
        {
        "nameen": "avatar62",
        "namezh": "燕玲媛"
        },

];

var award_config = {
    "award01": 1,
    "award02": 5,
    "award03": 8,
    "award04": 15//抽奖次数
};

// 初始化数据
//初始化抽奖号
if (!localStorage.getItem('lottery_initial')) {
    var data_str = JSON.stringify(lottery_initial_datas);
    localStorage.setItem('lottery_initial', data_str);
}
//初始化抽奖次数
if (!localStorage.getItem('award_initial')) {
    var award_str = JSON.stringify(award_config);
    localStorage.setItem('award_initial', award_str);
}