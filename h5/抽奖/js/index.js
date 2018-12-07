$('.start').click(function (){
	startLottery()
})

 var nextFrame = window.requestAnimationFrame       ||
                    window.webkitRequestAnimationFrame ||
                    window.mozRequestAnimationFrame    ||
                    window.msRequestAnimationFrame     ||
                    function(callback) {
                        var currTime = + new Date,
                                delay = Math.max(1000/60, 1000/60 - (currTime - lastTime));
                        lastTime = currTime + delay;
                        return setTimeout(callback, delay);
                    },
                    cancelFrame = window.cancelAnimationFrame         ||
                            window.webkitCancelAnimationFrame         ||
                            window.webkitCancelRequestAnimationFrame  ||
                            window.mozCancelRequestAnimationFrame     ||
                            window.msCancelRequestAnimationFrame      ||
                            clearTimeout,
                    // 初始滚动速度
                    speed = 3,
                    // 每个对话框外部高度(包括padding与margin)
                    // 注：为了方便，这里统一设置为 132+28 = 160
                    item_outer_height = $('.lottery-list:eq(1)').outerHeight(true),
                    item_height = $('.lottery-list:eq(1)').outerHeight(),
                    // 单个抽奖框框的中间位置
                    left_center_top = item_height/2 - 20,
                    // 抽奖按钮
                    lottery_btn = $('#lottery-btn'),
                    // 是否移动
                    isMove = true,
                    // 抽奖是否开始
                    isStart = false,
                    // 设置抽奖锁
                    isLock = true,
                    // 是否可以关闭重开
                    can_stop = false,
                    // 初始移动是的定时动画变量
                    timer = null,
                    // 全局 setTimeout 定时任务指定变量
                    setout_time = null;

function justGo(isMove) {
	 // $(".one").animate({"top":'-564px'},800,function () {
            
  //       });
  var moveDom = document.getElementById('lottery-main');
  console.log(moveDom);
  var move_height = moveDom.offsetHeight;
  var moveTop = moveDom.offsetTop;

  var all_size = 500
  var start_index = Math.floor(Math.random() * (all_size - 4))
  var start_top = -100 * start_index;

  var moveY = start_top

  $('#lottery-main').html($('#lottery-main').html() + $('#lottery-main').html());
  var justMove = function(flag){
  	
        timer = nextFrame(function() {
            moveY -= speed;
            moveDom.style.top = moveY + 'px';
            if (-(moveY) >= move_height) {
                moveY = 0;
            }
            justMove(flag);
        });
    };



  	if(isMove){
  		justMove(isMove)
  	}else {
  		return false
  	}
}

justGo(true)


function startLottery(){
	isStart = true;
	isLock = true;
	isMove = false;

	setout_time = setTimeout(function(){
		speed = 10;
	}, 1000);

	setout_time = setTimeout(function(){
		speed = 25;
	}, 2000);

	setout_time = setTimeout(function(){
		speed = 40;
	}, 3000);

	setout_time = setTimeout(function(){
		speed = 55;
	}, 4000);
	setout_time = setTimeout(function(){
		speed = 70;
		isLock = false;
	}, 5000);
}


