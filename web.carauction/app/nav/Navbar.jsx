import React from 'react';
import { FaCarAlt } from "react-icons/fa";

export default function Navbar() {
  return (
    <header className='sticky top-0 z-50 flex justify-between bg-white p-5 items-center text-gray-500 shadow-md'>
        <div className='flex items-center gap-2 text-3xl font-semibold text-red-500'>
            <FaCarAlt />
            <div>Car Auction</div>
        </div>
        <div>Search</div>
        <div>Login</div>
    </header>
  )
}
