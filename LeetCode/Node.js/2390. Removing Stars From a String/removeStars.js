/**
 * @param {string} s
 * @return {string}
 */
var removeStars = function (s) {
    let ansArray = []
    let i = s.length
    let skip = 0

    while (i--) {
        let char = s.charAt(i)

        if (char === '*') {
            skip++
            continue
        }

        if (char !== '*' && skip > 0) {
            skip--
            continue
        }
        ansArray.push(char)
    }
    // console.debug('in')
    // console.debug(ansArray)
    return ansArray.reverse().join('')
};

export default removeStars