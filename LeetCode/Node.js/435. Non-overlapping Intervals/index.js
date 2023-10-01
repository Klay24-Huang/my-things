var eraseOverlapIntervals = function (intervals) {
    if (intervals.length === 1) return 0
    intervals.sort((x, y) => {
        if (x[0] < y[0]) return -1
        if (x[0] > y[0]) return 1
        if (x[0] === y[0]) {
            if (x[1] < y[1]) return -1
            if (x[1] > y[1]) return 1
            return 0
        }
        return 0
    })

    const dp = Array(intervals.length).fill(0)
    for (let i = 1; i < intervals.length; i++) {
        const currItem = intervals[i]
        let arr = []
        for (let j = i - 1; j >= 0; j--) {
            const item = intervals[j]
            // // the same
            // if (item[0] == currItem[0] && item[1] == currItem[1]) {
            //     arr.push(1 + (i - j - 1) + dp[j])
            //     continue
            // }

            // overlap || the same
            if (item[0] <= currItem[0] && currItem[0] < item[1]) {
                // remove one item and interval item plus prev
                arr.push(1 + (i - j - 1) + dp[j])
                continue
            }

            // not overlap
            if (item[1] <= currItem[0]) {
                arr.push((i - j - 1) + dp[j])
                break
            }
        }
        // console.log("temp arr and index", arr, i)
        dp[i] = Math.min(...arr)
        // console.log('round end dp', dp)
    }
    // console.log('before return dp', dp)
    return dp[dp.length - 1]
};

export default eraseOverlapIntervals