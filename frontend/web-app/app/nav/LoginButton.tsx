'use client'

import { Button } from 'flowbite-react'
import React from 'react'
import {signIn} from 'next-auth/react';

export default function LoginButton() {
  return (
    <Button outline onClick={() => signIn('id-server', {callbackUrl: 'http://localhost:3000'}, {prompt: 'login'})}>
        Login
    </Button>
  )
}
