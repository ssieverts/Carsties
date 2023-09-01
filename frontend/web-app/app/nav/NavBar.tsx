import React from 'react'
import SearchBar from './SearchBar';
import Logo from './Logo';

export default function NavBar() {
  return (
    <header className='sticky top-0 flex justify-between bg-white p-5 text-gray-800 shadow-md'>
      <Logo />
      <SearchBar /> 
      <div>Login</div>
    </header>
  )
}
