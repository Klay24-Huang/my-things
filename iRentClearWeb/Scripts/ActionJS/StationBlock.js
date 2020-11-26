Slider.prototype.isSwipe = function (threshold) {
    return Math.abs(deltaX) > Math.max(threshold, Math.abs(deltaY));
}


Slider.prototype.touchStart = function (e) {

    if (this._isSliding) return false;

    touchMoving = true;
    deltaX = deltaY = 0;

    if (e.originalEvent.touches.length === 1) {

        startX = e.originalEvent.touches[0].pageX;
        startY = e.originalEvent.touches[0].pageY;

        this._$slider.on('touchmove touchcancel', this.touchMove.bind(this)).one('touchend', this.touchEnd.bind(this));

        isFlick = true;

        window.setTimeout(function () {
            isFlick = false;
        }, flickTimeout);
    }
}


Slider.prototype.touchMove = function (e) {

    deltaX = startX - e.originalEvent.touches[0].pageX;
    deltaY = startY - e.originalEvent.touches[0].pageY;

    if (this.isSwipe(swipeThreshold)) {
        e.preventDefault();
        e.stopPropagation();
        swiping = true;
    }
    if (swiping) {
        this.slide(deltaX / this._sliderWidth, true)
    }
}


Slider.prototype.touchEnd = function (e) {

    var threshold = isFlick ? swipeThreshold : this._sliderWidth / 2;

    if (this.isSwipe(threshold)) {
        deltaX < 0 ? this.prev() : this.next();
    }
    else {
        this.slide(0, !deltaX);
    }

    swiping = false;

    this._$slider.off('touchmove', this.touchMove).one(transitionend, $.proxy(function () {
        this.slide(0, true);
        touchMoving = false;
    }, this));
}