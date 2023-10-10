import React from "react";
import SearchBar from "./SearchBar";
import Logo from "./Logo";
import LoginButton from "./LoginButton";
import { getCurrentUser } from "../actions/authActions";
import UserActions from "./UserActions";

export default async function NavBar() {
  const user = await getCurrentUser();

  return (
    <header className="sticky top-0 flex justify-between bg-white p-5 text-gray-800 shadow-md">
      <Logo />
      <SearchBar />
      {user ? <UserActions user={user} /> : <LoginButton />}
    </header>
  );
}
