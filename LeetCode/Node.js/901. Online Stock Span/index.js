var StockSpanner = function () {
    this.index = -1
    this.stack = []
};

/** 
 * @param {number} price
 * @return {number}
 */
StockSpanner.prototype.next = function (price) {
    this.index++

    while (this.stack.length > 0 && this.stack[0][0] <= price) {
        this.stack.shift()
    }

    if (this.stack.length === 0) {
        this.stack.push([price, this.index])
        return this.index + 1
    }

    if (this.stack[0][0] > price) {
        this.stack.unshift([price, this.index])
        // console.log("stack", this.stack)
        return this.index - this.stack[1][1]
    }
};