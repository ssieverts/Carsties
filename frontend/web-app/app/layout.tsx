import { Metadata } from 'next'
import './globals.css'
import NavBar from './nav/NavBar'

export const metadata: Metadata = {
  title: 'Carsties',
  description: 'Auto Auction Marketplace',
}

export default function RootLayout({
  children,
}: {
  children: React.ReactNode
}) {
  return (
    <html lang="en">
      <body>
        <NavBar />
        <main className='container mx-auto px-5 pt-10'>
          {children}
        </main>
      </body>
    </html>
  )
}
