 $(function() {
            
            var drawerTab = {
                
                speed:500,
                containerWidth:$('.calendar-panel').outerWidth(),
                containerHeight:$('.calendar-panel').outerHeight(),
                tabWidth:$('.calendar-tab').outerWidth(),
                
                init:function(){
					
                    $('.calendar-panel').css('height',drawerTab.containerHeight + 'px');
                    
                    $('a.calendar-tab').click(function(event){
            			var cpz=$('.calendar-panel').css('z-index')
						$('.calendar-panel').css({'z-index':cpz+1})
                        if ($('.calendar-panel').hasClass('open')) {
                            $('.calendar-panel').animate({left:'-' + drawerTab.containerWidth}, drawerTab.speed)
                            .removeClass('open');
                        } else {
                            $('.calendar-panel').animate({left:'225'},  drawerTab.speed)
                            .addClass('open');
                        }
                        event.preventDefault();
                    });
                }
            };
            
            drawerTab.init();
        });
	    $(function() {
            
            var drawerTab = {
                
                speed:500,
                containerWidth:$('.tetris-panel').outerWidth(),
                containerHeight:$('.tetris-panel').outerHeight(),
                tabWidth:$('.tetris-tab').outerWidth(),
                
                init:function(){
                    $('.tetris-panel').css('height',drawerTab.containerHeight + 'px');
                    
                    $('a.tetris-tab').click(function(event){
            			var tpz=$('.tetris-panel').css('z-index')
						$('.tetris-panel').css({'z-index':tpz+1})
                        if ($('.tetris-panel').hasClass('open')) {
                            $('.tetris-panel').animate({left:'-' + drawerTab.containerWidth}, drawerTab.speed)
                            .removeClass('open');
                        } else {
                            $('.tetris-panel').animate({left:'225'},  drawerTab.speed)
                            .addClass('open');
                        }
                        event.preventDefault();
                    });
                }
            };
            
            drawerTab.init();
        });