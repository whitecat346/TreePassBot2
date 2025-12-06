// Test file to verify @services/api import
import { getBotStatus, startBot, stopBot } from '@/services/api';

console.log('API import test:');
console.log('getBotStatus:', typeof getBotStatus);
console.log('startBot:', typeof startBot);
console.log('stopBot:', typeof stopBot);
