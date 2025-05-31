import Axios from 'axios'
import { helloWorld } from '@alias/lib'

helloWorld()
console.log('Hello World from index.ts!')

const res = await Axios.get('https://jsonplaceholder.typicode.com/posts/1')
console.log(res.data)
