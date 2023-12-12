export {default} from 'next-auth/middleware'

export const config = {
    matcher: [
        '/sessions'
    ],
    pages: {
        signIn: '/api/auth/signin'
    }
}