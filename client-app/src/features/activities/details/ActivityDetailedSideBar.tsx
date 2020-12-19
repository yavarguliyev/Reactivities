import { observer } from 'mobx-react-lite';
import React, { Fragment } from 'react';
import { Link } from 'react-router-dom';
import { Segment, List, Item, Label, Image } from 'semantic-ui-react';
import { IAttendee } from '../../../app/models/activity';

interface IProps {
  attendess: IAttendee[];
}

const ActivityDetailedSideBar: React.FC<IProps> = ({ attendess }) => {
  
  return (
    <Fragment>
      <Segment textAlign='center' style={{ border: 'none' }} attached='top' secondary inverted color='teal'>
        {attendess.length} {attendess.length === 1 ? 'Person' : 'People'} going
      </Segment>
      <Segment attached>
        <List relaxed divided>
          {attendess.map(attendee => (
            <Item key={attendee.userName} style={{ position: 'relative' }}>
              {attendee.isHost && <Label style={{ position: 'absolute' }} color="orange" ribbon="right">Host</Label>}
              <Image size="tiny" src={attendee.image || `/assets/user.png`} />
              <Item.Content verticalAlign='middle'>
                <Item.Header>
                  <Link to={`/profile/${attendee.userName}`}>{attendee.displayName}</Link>
                </Item.Header>
                <Item.Extra style={{ color: 'orange' }}>Following</Item.Extra>
              </Item.Content>
            </Item>
          ))}
        </List>
      </Segment>
    </Fragment>
  )
}

export default observer(ActivityDetailedSideBar);
