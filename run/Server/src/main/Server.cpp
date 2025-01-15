#include "Server.hpp"

#include <iostream>

namespace manila {
	void Server::init() { std::cout << "Server::init()" << std::endl; }
}

int main() {
	manila::Server::init();

	return 0;
}
