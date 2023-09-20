package solution

func predictPartyVictory(senate string) string {
	banQ := newQueue()
	restQ := newQueue()
	for _, rune := range senate {
		prev, ok := banQ.peek()

		if !ok {
			// empty
			banQ.enqueue(rune)
			continue
		}

		// ban
		if prev != rune {
			banQ.dequeue()
			restQ.enqueue(prev)
			continue
		}

		// same
		if prev == rune {
			banQ.enqueue(rune)
			continue
		}
	}

	dic := map[rune]string{
		'R': "Radiant",
		'D': "Dire",
	}
	count := banQ.getLength() + restQ.getLength()
	if count == 1 || count == len(senate) {
		fromBan, ok := banQ.peek()
		if ok {
			return dic[fromBan]
		}

		fromRest, ok := restQ.peek()
		if ok {
			return dic[fromRest]
		}
	}

	return predictPartyVictory(string(append(banQ.array, restQ.array...)))
}

type Queue struct {
	array []rune
}

func newQueue() Queue {
	return Queue{
		array: make([]rune, 0),
	}

}

func (q *Queue) enqueue(item rune) {
	q.array = append(q.array, item)
}

func (q *Queue) dequeue() (rune, bool) {
	if q.getLength() == 0 {
		return 0, false
	}
	item := q.array[0]
	q.array = q.array[1:]
	return item, true
}

func (q *Queue) peek() (rune, bool) {
	if q.getLength() == 0 {
		return 0, false
	}

	return q.array[0], true
}

func (q *Queue) getLength() int {
	return len(q.array)
}
