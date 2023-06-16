package user

type UserService struct {
	userRepository *UserRepository
}

func NewService(userRepository *UserRepository) *UserService {
	return &UserService{userRepository: userRepository}
}

func (u *UserService) GetById(id int) User {
	return u.userRepository.GetById(id)
}
