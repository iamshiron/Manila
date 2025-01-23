#include "Server.hpp"

#include <iostream>

namespace manila {
	void Server::init() { std::cout << "Server::init()" << std::endl; }
}

int main() {
	manila::Server::init();

	std::cout << "Hello, World!" << std::endl;
	std::cout << "Goodbye, World!" << std::endl;

	return -1;
}
