# New Server Operation Mode for Distributed Work

The program will run in server mode, responding to HTTP requests.

This will enable distributed work and allow handling large amounts of information.

Besides this, some minor fixes and improvements have been made:

1. Fixed nullability check of the previous bucket due to inverted logic;
    > During the tree navigation reimplementation, a check was added that only works for bucket navigation. When selecting a single bucket, it failed due to inverted logic.
2. Increased page navigation count to allow selecting data from the previous four months;
    > The program previously navigated only two pages back, but to retrieve 90 days of historical data, if the fourth month is at the beginning, two pages are not enough.
3. Fixed the regex used to retrieve resource numbers;
    > The previous regex only captured numbers between spaces.
4. Now the program operates in only one mode at a time;
    > Instead of executing features sequentially, the program now runs in a single mode at a time.
