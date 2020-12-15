import React, { FormEvent, useState } from 'react';
import { Segment, Form, Button } from 'semantic-ui-react';
import { IActivity } from '../../app/models/activity';
import {v4 as uuid} from 'uuid';

interface Props {
  setEditMode: (editMode: boolean) => void;
  activity: IActivity;
  createActivity: (activity: IActivity) => void;
  editActivity: (activity: IActivity) => void;
}

const ActivityForm: React.FC<Props> = ({ setEditMode, activity: initialFormState, createActivity, editActivity }) => {
  const initializeForm = () => {
    if (initialFormState) {
      return initialFormState;
    } else {
      return {
        id: '',
        title: '',
        description: '',
        category: '',
        date: '',
        city: '',
        venue: ''
      }
    }
  };

  const [activity, setActivity] = useState<IActivity>(initializeForm);

  const handleInputOnChange = (event: FormEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    const {name, value} = event.currentTarget;
    setActivity({...activity, [name]: value});
  }

  const handleSubmit = () => {
    if(activity.id.length === 0) {
      let newActivity = {
        ...activity,
        id: uuid()
      }
      createActivity(newActivity);
    } else {
      editActivity(activity);
    }
  }

  return (
    <Segment clearing>
      <Form onSubmit={handleSubmit}>
        <Form.Input onChange={handleInputOnChange} name="title" placeholder="Title" value={activity.title} />
        <Form.TextArea onChange={handleInputOnChange} name="description" placeholder="Description" value={activity.description} />
        <Form.Input onChange={handleInputOnChange} name="category" placeholder="Category"  value={activity.category}/>
        <Form.Input onChange={handleInputOnChange} name="date" type="datetime-local" placeholder="Date"  value={activity.date}/>
        <Form.Input onChange={handleInputOnChange} name="city" placeholder="City" value={activity.city} />
        <Form.Input onChange={handleInputOnChange} name="venue" placeholder="Venue"  value={activity.venue}/>
        <Button floated="right" positive type="submit" content="Submit" />
        <Button onClick={() => setEditMode(false)} floated="right" type="button" content="Cancel" />
      </Form>
    </Segment>
  )
}

export default ActivityForm
