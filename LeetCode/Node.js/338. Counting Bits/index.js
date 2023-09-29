var countBits = function (n) {
    let ans = []
    for (let i = 0; i <= n; i++) {
        ans.push(i.toString(2).split(1).length - 1)
    }
    return ans
}

export default countBits