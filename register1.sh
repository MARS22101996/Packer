#!/bin/bash
register(){
   data='{"ID":"'"$1"'","Name":"'"$1"'","Tags":["urlprefix-/'"$1"'"],"Address":"'"$(hostname -I | sed 's/ .*//')"'","Port":'"$2"',"Check":{"http":"http://'"$(hostname -I | sed 's/ .*//')"':'"$2"'/'"$1"'/status","Interval":"10s"}}'
   request='curl -X PUT -H "Content-Type: application/json" --data '"'"''"$data"''"'"'  http://'"$(hostname -I | sed 's/ .*//')"':8500/v1/agent/service/register'

   eval $request
}

register 'UserService' '5001'
register 'TeamService' '5002'
register 'TicketService' '5003'
register 'StatisticService' '5004'
register 'NotificationService' '5006'