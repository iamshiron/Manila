#include "Server.hpp"

#include "../../../Core/src/main/Core.hpp"

#include <iostream>

namespace manila {
	void Server::init() { std::cout << "Server::init()" << std::endl; }
}

int main() {
	manila::Server::init();
	manila::Core::init();

	std::cout << "Hello, World!" << std::endl;
	std::cout << "Goodbye, World!" << std::endl;

	return 0;
}
