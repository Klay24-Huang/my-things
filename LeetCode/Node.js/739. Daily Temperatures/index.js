/**
 * @param {number[]} temperatures
 * @return {number[]}
 */
var dailyTemperatures = function (temperatures) {
    const ans = Array(temperatures.length).fill(0)
    const stack = []
    for (let i = temperatures.length - 1; i >= 0; i--) {
        // console.log('index', i)
        while (stack.length > 0 && temperatures[i] >= temperatures[stack[0]]) {
            stack.shift()
            // console.log('pop stack', ans, stack)
        }

        if (stack.length === 0) {
            stack.push(i)
            ans[i] = 0
            // console.log('add stack', ans, stack)
            continue
        }

        if (temperatures[i] < temperatures[stack[0]]) {
            ans[i] = stack[0] - i
            stack.unshift(i)
            // console.log('add stack', ans, stack)
            continue
        }
        // console.log('after round', ans, stack)
    }
    // console.log('before return', ans, stack)
    return ans
};

export default dailyTemperatures