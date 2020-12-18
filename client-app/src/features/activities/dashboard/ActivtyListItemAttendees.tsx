import React from 'react';
import { List, Image, Popup } from 'semantic-ui-react';
import { IAttendee } from '../../../app/models/activity';

interface IProps {
  attendees: IAttendee[];
}

const ActivtyListItemAttendees: React.FC<IProps> = ({ attendees }) => {
  return (
    <List horizontal>
      {attendees.map(attendee => (
        <List.Item key={attendee.userName}>
          <Popup header={attendee.userName} trigger={
            <Image size='mini' circular src={attendee.image || '/assets/user.png'}/>
          } />
        </List.Item>
      ))}
    </List>
  )
}

export default ActivtyListItemAttendees;
