var Trie = function (isWord = false) {
    this.isWord = isWord
    this.nodes = new Map()
};

/** 
 * @param {string} word
 * @return {void}
 */
Trie.prototype.insert = function (word) {
    let currTrie = this
    // console.log(currTrie)
    for (const w of word) {
        if (!currTrie.nodes.has(w)) {
            const newTrie = new Trie()
            currTrie.nodes.set(w, newTrie)
            currTrie = newTrie
            continue
        }
        currTrie = currTrie.nodes.get(w)
    }
    currTrie.isWord = true
};

/** 
 * @param {string} word
 * @return {boolean}
 */
Trie.prototype.search = function (word) {
    let currTrie = this
    for (const w of word) {
        // console.log('search trie', currTrie)
        if (!currTrie.nodes.has(w)) return false
        currTrie = currTrie.nodes.get(w)
    }
    return currTrie.isWord
};

/** 
 * @param {string} prefix
 * @return {boolean}
 */
Trie.prototype.startsWith = function (prefix) {
    let currTrie = this
    for (const w of prefix) {
        if (!currTrie.nodes.has(w)) return false
        currTrie = currTrie.nodes.get(w)
    }
    return true
};

/** 
 * Your Trie object will be instantiated and called as such:
 * var obj = new Trie()
 * obj.insert(word)
 * var param_2 = obj.search(word)
 * var param_3 = obj.startsWith(prefix)
 */

export default Trie