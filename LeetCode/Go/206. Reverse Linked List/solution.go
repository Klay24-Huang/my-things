package solution

type ListNode struct {
	Val  int
	Next *ListNode
}

func reverseList(head *ListNode) *ListNode {
	var newLL *ListNode
	flag := true
	for flag {
		if head == nil {
			break
		}

		next := head.Next
		temp := newLL
		newLL = head
		newLL.Next = temp
		head = next
	}

	return newLL
}
